using SolutionForSubtitleTimeshift.Interfaces;
using SolutionForSubtitleTimeshift.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SolutionForSubtitleTimeshift.Implementations
{
  public class SubtitleToModelProcessor : ISubtitleToModelProcessor
  {
    public async Task<IEnumerable<SubtitleModel>> GenerateModel(Stream stream, Encoding encoding, int buffersize, bool leaveOpen)
    {
      var structureSubtitleModel = new List<SubtitleModel>();
      var detectBOM = true;

      using (var streamReader = new StreamReader(stream, encoding, detectBOM, buffersize, leaveOpen))
      {
        while (!streamReader.EndOfStream)
        {
          var subtitleModel = new SubtitleModel();

          var (found, content, emptyLines) = await IsASectionStart(streamReader);
          subtitleModel.OrderWhiteLine = emptyLines;
          
          if (found)
            subtitleModel.Order = int.Parse(content);
          
          (found, content, emptyLines) = await IsATimeSpanSection(found, content, streamReader);
          subtitleModel.TimeSpanWhiteLines = emptyLines;

          if (found)
            (subtitleModel.Start, subtitleModel.End) = ExtractSubtitleTimeSpan(content);

          subtitleModel.Text = await ExtractSubtitleText(found, content, streamReader);

          if (found)
            structureSubtitleModel.Add(subtitleModel);
        }
      }

      return structureSubtitleModel;
    }

    private async Task<(bool, string, int)> IsASectionStart(StreamReader streamReader)
    {
      var (content, emptyLinesFound) = await WhiteLineCounter(streamReader);

      var itIs = int.TryParse(content, out var sectionStart);

      return (itIs, content, emptyLinesFound);
    }

    private async Task<(bool, string, int)> IsATimeSpanSection(bool previousOk, string previousContent, StreamReader streamReader)
    {
      string content;
      int emptyLinesFound = 0;

      if (previousOk)
        (content, emptyLinesFound) = await WhiteLineCounter(streamReader);
      else
        content = previousContent;

      var itIsTimeSpan1 = Regex.IsMatch(content, @"^[0-9]{1,2}[:][0-9]{1,2}[:][0-9]{1,2}[, | .][0-9]{3,7}");
      var itIsTimeSpan2 = Regex.IsMatch(content, @"[0-9]{1,2}[:][0-9]{1,2}[:][0-9]{1,2}[, | .][0-9]{3,7}$");

      if (itIsTimeSpan1 && itIsTimeSpan2)
      {
        return (true, content,  emptyLinesFound);
      }

      return (false, content, emptyLinesFound);
    }

    private (TimeSpan, TimeSpan) ExtractSubtitleTimeSpan(string timeSpanLine)
    {
      TimeSpan startTimeSpan;
      TimeSpan endTimeSpan;

      timeSpanLine = timeSpanLine.Trim('\r', '\n', ' ');

      try
      {
        var startTime = Regex.Matches(timeSpanLine, @"^[0-9]{1,2}[:][0-9]{1,2}[:][0-9]{1,2}[, | .][0-9]{3,7}")[0].ToString().Replace(",", ".");
        var endTime = Regex.Matches(timeSpanLine, @"[0-9]{1,2}[:][0-9]{1,2}[:][0-9]{1,2}[, | .][0-9]{3,7}$")[0].ToString().Replace(",", ".");

        startTimeSpan = TimeSpan.Parse(startTime);
        endTimeSpan = TimeSpan.Parse(endTime);
      }
      catch (Exception ex)
      {
        throw new InvalidDataException("Invalid Data: Incorrect format for date.", ex);
      }


      return (startTimeSpan, endTimeSpan);
    }

    private async Task<List<string>> ExtractSubtitleText(bool previousOk, string previousContent, StreamReader streamReader)
    {
      var textSection = new List<string>();
      string textLine;

      if (!previousOk)
        textSection.Add(previousContent);

      do
      {
        textLine = await streamReader.ReadLineAsync();
        textSection.Add(textLine);

      } while (!string.IsNullOrEmpty(textLine));

      return textSection;
    }

    private async Task<(string, int)> WhiteLineCounter(StreamReader streamReader)
    {
      string line = null;
      
      int lineCounter = 0;
      while (string.IsNullOrEmpty(line) && !streamReader.EndOfStream)
      {
        line = await streamReader.ReadLineAsync();
        lineCounter++;
      }

      return (line, lineCounter - 1);
    }
  }
}
