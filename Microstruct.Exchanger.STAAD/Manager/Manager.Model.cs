using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microstruct.Prime.Base.Media;

namespace Microstruct.Exchanger.STAAD
{
  public partial class Manager
  {
    public partial class Model
    {
      #region Data

      public static Dataset_AsModel Content { get; set; } = new();

      #endregion   
    }
  }
}
