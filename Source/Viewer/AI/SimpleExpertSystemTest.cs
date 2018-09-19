using System;
using System.Collections.Generic;
using System.Diagnostics;

using SimpleExpertSystem;

namespace Dataweb.NShape.Viewer.AI
{
	static class SimpleExpertSystemTest
	{
		private const string Bicycle = "Bicycle";
		private const string Tricycle = "Tricycle";
		private const string Motorcycle = "Motorcycle";
		private const string SportsCar = "SportsCar";
		private const string Sedan = "Sedan";
		private const string MiniVan = "MiniVan";
		private const string SUV = "SUV";
		private const string Cycle = "Cycle";   // велосипед
		private const string Automobile = "Automobile"; // автомобиль

		static void Run()
		{
		//	Console.WriteLine("DemoBackwardChainWithNullMemory");
		//	DemoBackwardChainWithNullMemory();

			Console.WriteLine("TestForwardChain");
			TestForwardChain();

			Console.WriteLine("TestBackwardChain");
			TestBackwardChain();
		}

		static private RuleInferenceEngine getInferenceEngine()
		{
			RuleInferenceEngine rie = new RuleInferenceEngine();

			Rule rule = new Rule(Bicycle);
			rule.AddAntecedent(new ClauseIs("vehicleType", "cycle"));
			rule.AddAntecedent(new ClauseIs("num_wheels", "2"));
			rule.AddAntecedent(new ClauseIs("motor", "no"));
			rule.Consequent = new ClauseIs("vehicle", Bicycle);
			rie.AddRule(rule);

			rule = new Rule(Tricycle);
			rule.AddAntecedent(new ClauseIs("vehicleType", "cycle"));
			rule.AddAntecedent(new ClauseIs("num_wheels", "3"));
			rule.AddAntecedent(new ClauseIs("motor", "no"));
			rule.Consequent = new ClauseIs("vehicle", Tricycle);
			rie.AddRule(rule);

			rule = new Rule(Motorcycle);
			rule.AddAntecedent(new ClauseIs("vehicleType", "cycle"));
			rule.AddAntecedent(new ClauseIs("num_wheels", "2"));
			rule.AddAntecedent(new ClauseIs("motor", "yes"));
			rule.Consequent = new ClauseIs("vehicle", Motorcycle);
			rie.AddRule(rule);

			rule = new Rule(SportsCar);
			rule.AddAntecedent(new ClauseIs("vehicleType", "automobile"));
			rule.AddAntecedent(new ClauseIs("size", "medium"));
			rule.AddAntecedent(new ClauseIs("num_doors", "2"));
			rule.Consequent = new ClauseIs("vehicle", SportsCar);
			rie.AddRule(rule);

			rule = new Rule(Sedan);
			rule.AddAntecedent(new ClauseIs("vehicleType", "automobile"));
			rule.AddAntecedent(new ClauseIs("size", "medium"));
			rule.AddAntecedent(new ClauseIs("num_doors", "4"));
			rule.Consequent = new ClauseIs("vehicle", Sedan);
			rie.AddRule(rule);

			rule = new Rule(MiniVan);
			rule.AddAntecedent(new ClauseIs("vehicleType", "automobile"));
			rule.AddAntecedent(new ClauseIs("size", "medium"));
			rule.AddAntecedent(new ClauseIs("num_doors", "3"));
			rule.Consequent = new ClauseIs("vehicle", MiniVan);
			rie.AddRule(rule);

			rule = new Rule(SUV);
			rule.AddAntecedent(new ClauseIs("vehicleType", "automobile"));
			rule.AddAntecedent(new ClauseIs("size", "large"));
			rule.AddAntecedent(new ClauseIs("num_doors", "4"));
			rule.Consequent = new ClauseIs("vehicle", SUV);
			rie.AddRule(rule);

			rule = new Rule(Cycle);
			rule.AddAntecedent(new ClauseLt("num_wheels", "4"));
			rule.Consequent = new ClauseIs("vehicleType", "cycle");
			rie.AddRule(rule);

			rule = new Rule(Automobile);
			rule.AddAntecedent(new ClauseIs("num_wheels", "4"));
			rule.AddAntecedent(new ClauseIs("motor", "yes"));
			rule.Consequent = new ClauseIs("vehicleType", "automobile");
			rie.AddRule(rule);

			return rie;
		}

		static public void DemoBackwardChainWithNullMemory()
		{
			RuleInferenceEngine rie = getInferenceEngine();

			Console.WriteLine("Infer with All Facts Cleared:");
			rie.ClearFacts();

			List<Clause> unproved_conditions = new List<Clause>();

			Clause conclusion = null;
			while(conclusion == null)
			{
				conclusion = rie.Infer("vehicle", unproved_conditions);
				if(conclusion == null)
				{
					if(unproved_conditions.Count == 0)
						break;
					Clause c = unproved_conditions[0];
					Console.WriteLine("ask: " + c + "?");
					unproved_conditions.Clear();
					Console.WriteLine("What is " + c.Variable + "?");
					String value = Console.ReadLine();
					rie.AddFact(new ClauseIs(c.Variable, value));
				}
			}

			Console.WriteLine("Conclusion: " + conclusion);
			Console.WriteLine("Memory: ");
			Console.WriteLine("{0}", rie.Facts);
		}

		static public void TestForwardChain()
		{
			RuleInferenceEngine rie = getInferenceEngine();
			rie.AddFact(new ClauseIs("num_wheels", "4"));
			rie.AddFact(new ClauseIs("motor", "yes"));
			rie.AddFact(new ClauseIs("num_doors", "3"));
			rie.AddFact(new ClauseIs("size", "medium"));

			Console.WriteLine("before inference");
			Console.WriteLine("{0}", rie.Facts);
			Console.WriteLine("");

			rie.Infer(); //forward chain

			Console.WriteLine("after inference");
			Console.WriteLine("{0}", rie.Facts);
			Console.WriteLine("");

			Debug.Assert(rie.Facts.Count == 6);
		}

		static public void TestBackwardChain()
		{
			RuleInferenceEngine rie = getInferenceEngine();
			rie.AddFact(new ClauseIs("num_wheels", "4"));
			rie.AddFact(new ClauseIs("motor", "yes"));
			rie.AddFact(new ClauseIs("num_doors", "3"));
			rie.AddFact(new ClauseIs("size", "medium"));

			Console.WriteLine("Infer: vehicle");

			List<Clause> unproved_conditions = new List<Clause>();

			Clause conclusion = rie.Infer("vehicle", unproved_conditions);

			Console.WriteLine("Conclusion: " + conclusion);

			Debug.Assert(conclusion.Value == MiniVan);
		}
	}
}
