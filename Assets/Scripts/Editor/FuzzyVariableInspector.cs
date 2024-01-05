using UnityEditor;

namespace GCU.FraserConnolly.AI.Fuzzy
{
	[CustomEditor(typeof(FuzzyVariable))]
	public class FuzzyVariableInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var flv = target as FuzzyVariable;
			flv.OnValidate();
			TLP.Editor.EditorGraph graph = new TLP.Editor.EditorGraph(flv.MinRange, 0f, flv.MaxRange, 1f, "Fuzzy Sets", FuzzyModule.NumSamples);

			graph.AddLineX(flv.CrispValue, color: UnityEngine.Color.green);

			// render grid lines
			graph.GridLinesX = (flv.MaxRange - flv.MinRange) / 10;
			graph.GridLinesY = .25f;

			foreach (var item in flv.GetSets())
			{
				if (item == null)
				{
					continue;
				}
				graph.AddFunction(x => (float)item.CalculateDOM(x), item.Colour);
				graph.AddLineX(item.GetRepresentativeVal(), item.Colour);
			}

			var stepSize = (flv.MaxRange - flv.MinRange) / FuzzyModule.NumSamples;

			float warningAreaStart = 0f;
			bool inWarningArea = false;

			for (var i = 0; i < FuzzyModule.NumSamples; i++)
			{
				var x = (stepSize * i) + flv.MinRange;
				var membership = flv.SumOfMembership(x);

				if (inWarningArea)
				{
					if (membership > 0.95f && membership < 1.05f)
					{
						inWarningArea = false;
						graph.AddWarningArea(warningAreaStart, x - warningAreaStart);
					}
				}
				else
				{
					if (membership < 0.95f || membership > 1.05f)
					{
						inWarningArea = true;
						warningAreaStart = x;
					}
				}
			}

			if (inWarningArea)
			{
				// close final warning area
				graph.AddWarningArea(warningAreaStart, flv.MaxRange - warningAreaStart);
			}

			graph.Draw();
		}
	}
}