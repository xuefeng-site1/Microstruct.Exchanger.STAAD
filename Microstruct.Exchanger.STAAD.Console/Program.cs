/*
 * MIT License

 * Copyright (c) [year] [copyright holders]

 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.

 * For anything more about this, please refer to www.coheng.com for the base program of Microstruct.
 * For any questions related to this usage, please send email to zxf315@gmail.com.

 * THE SOFTWARE IS PROVIDED "AS IS," WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES, OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT, OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.Runtime.InteropServices;

using Microstruct.Prime.Base.Math;
using OpenSTAADUI;

namespace Microstruct.Exchanger.STAAD
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        if (Marshal.GetActiveObject("StaadPro.OpenSTAAD") is OpenSTAAD os)
        {
          var geometry = os.Geometry;

          var node_count = geometry.GetNodeCount();
          var beam_count = geometry.GetMemberCount();

          Console.WriteLine("Unit = " + 1.0.InCM().ToM());
          Console.WriteLine("Node Count = " + node_count);
          Console.WriteLine("Beam Count = " + beam_count);

          Console.ReadLine();
        }
      }
      catch { }
    }
  }
}

