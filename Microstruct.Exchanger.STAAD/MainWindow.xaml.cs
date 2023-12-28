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
using System.Windows;
using System.Runtime.InteropServices;

using Microstruct.Prime.Base.Math;
using Microstruct.Prime.Base.Media;
using OpenSTAADUI;

namespace Microstruct.Exchanger.STAAD
{
  public partial class MainWindow : System.Windows.Window
  {
    #region Data

    public Unit.English Unit_World { get; set; } = Unit.English.New(i =>
    {
      i.Length_Bind = Unit.English.Length.FT;
      i.Mass_Bind = Unit.English.Mass.KIPM;
    });

    #endregion

    #region public MainWindow()

    public MainWindow()
    {
      InitializeComponent();
      this.Reset();
    }

    #endregion

    #region public void Reset()

    public void Reset()
    {
      this.Value_AsWorkspace.Reset(Manager.Model.Content);
    }

    #endregion

    #region private void OnCall_Executed(object sender, RoutedEventArgs e)

    private void OnCall_Executed(object sender, RoutedEventArgs e)
    {
      Manager.Model.Content.Initialize();

      try
      {
        if (Marshal.GetActiveObject("StaadPro.OpenSTAAD") is OpenSTAAD os)
        {
          //read current staad unit system
          var (unit_id, unit_staad) = this.Read_Unit(os);

          //read nodes from staad
          this.Read_Nodes(os, (unit_id, unit_staad));

          //read beams from staad
          this.Read_Beams(os);

          //msg
          this.Value_AsFooter.Content = $"Unit: {this.Unit_World}";
        }
      }
      catch
      {
        this.Value_AsFooter.Content = $"Make sure STAAD Pro is running!";
      }
    }

    #endregion

    #region private (int id, Unit.Base staad) Read_Unit(OpenSTAAD openstaad)

    private (int id, Unit.Base staad) Read_Unit(OpenSTAAD openstaad)
    {
      //call staad for the current unit system (1:English; 2:Metric)
      var unit_sign = openstaad.GetBaseUnit();

      //construct unit variable based on STAAD unit
      var unit_staad = unit_sign switch
      {
        1 => Unit.English.New(i =>
        {
          i.Length_Bind = Unit.English.Length.IN;
          i.Mass_Bind = Unit.English.Mass.KIPM;
        }) as Unit.Base,
        2 => Unit.Metric.New(i =>
        {
          i.Length_Bind = Unit.Metric.Length.M;
          i.Mass_Bind = Unit.Metric.Mass.T;
        }) as Unit.Base,
        _ => throw new NotImplementedException()
      };

      //create unit in the dataset
      var row = Manager.Model.Content.Unit.Create(i =>
      {
        i.IsEngineering(true);
        i.IsActive(true);
        i.IsLocked(false);

        i.Name("World");
        i.Value(this.Unit_World);
      });

      //returns current unit id in the dataset, and unit variable of STAAD
      return (row.Id(), unit_staad);
    }

    #endregion

    #region private void Read_Nodes(OpenSTAAD openstaad, (int id, Unit.Base staad) unit)

    private void Read_Nodes(OpenSTAAD openstaad, (int id, Unit.Base staad) unit)
    {
      //call geometry instance of STAAD
      var geometry = openstaad.Geometry;

      //get node count
      var count = geometry.GetNodeCount();

      //get node list
      var nodes = new int[count];

      //call STAAD to get the list of nodes
      geometry.GetNodeList(ref nodes);

      //create nodes
      nodes.ForEach(id =>
      {
        geometry.GetNodeCoordinates(id, out double x, out double y, out double z);

        //create node in dataset
        Manager.Model.Content.Node.Create(i =>
        {
          i.Id(id); //STAAD node id
          i.X(x.InLength(unit.staad).To(this.Unit_World)); //unit converted from STAAD to the user prefered "Unit_World" - 1st way (more to refer Microstruct.Prime.Base.Math)
          i.Y(y.In(unit.staad).ToLength(this.Unit_World)); //unit converted from STAAD to the user prefered "Unit_World" - 2nd way (more to refer Microstruct.Prime.Base.Math)
          i.Z(z.In(unit.staad).To(this.Unit_World, Unit.Type.Length)); //unit converted from STAAD to the user prefered "Unit_World" - 3rd way (more to refer Microstruct.Prime.Base.Math)
          i.Id_AsUnit(unit.id); //set unit link
        });
      });
    }

    #endregion

    #region private void Read_Beams(OpenSTAAD staad)

    private void Read_Beams(OpenSTAAD staad)
    {
      //call geometry instance of STAAD
      var geometry = staad.Geometry;

      //get beam count
      var count = geometry.GetMemberCount();

      //get beam list
      var beams = new int[count];

      //call STAAD to get the list of beams
      geometry.GetBeamList(ref beams);

      //create beams
      beams.ForEach(id =>
      {
        geometry.GetMemberIncidence(id, out int id1, out int id2);

        //create beam in dataset
        Manager.Model.Content.Beam02.Create(i =>
        {
          i.Id(id); //STAAD beam id
          i.Id1(id1); //STAAD beam - node id1
          i.Id2(id2); //STAAD beam - node id2
        });
      });
    }

    #endregion
  }
}
