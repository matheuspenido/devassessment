﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SolutionForSubtitleTimeshift.Models
{
  public class SubtitleModel
  {
    public SubtitleModel()
    {
      Text = new List<string>();
    }
    public int OrderWhiteLine { get; set; }
    public int Order { get; set; }
    public int TimeSpanWhiteLines { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public List<string> Text { get; set; }
  }
}
