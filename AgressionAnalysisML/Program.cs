using System;
using System.Linq;
using System.Collections.Generic;
using AgressionAnalysisMLML.Model.DataModels;
using Microsoft.ML;

namespace AgressionAnalysisML
{
    struct annotation
    {
        public decimal aggressionScore;
        public int numOfVotes;
        public string comment;
    }

    class Program
    {
        
        static void Main(string[] args)
        {
            //All Aggression Data was taken from the wikipedia Detox project: https://meta.wikimedia.org/wiki/Research:Detox/Data_Release#Aggression         
            //The Files for Agression data are found at https://figshare.com/articles/Wikipedia_Talk_Labels_Aggression/4267550

            //Step 1. TSVMerge(..) was used to Merge aggression_annotations.tsv & aggression_annotated_comments.tsv data into a single TSV file aggression_merged.tsv
            //TSVMerge("G:\SentimentAnalysis\Aggression"); 

            //Step 2. A model was trained using aggression_merged.tsv for 600 seconds and MLModel.zip was generated.

            //Step3. Load the model
            MLContext mlContext = new MLContext();

            ITransformer mlModel = mlContext.Model.Load("MLModel.zip", out var modelInputSchema);

            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            //Step3. Pick Test Data
            string[] testData =
            {
                "This is very bad stuff, i don't like it.",
                "This is very good stuff, i love it!",
                "from what i can see, the job was completed and everything is fine",
                "there is no way you will be able to complete the job on time",
            };
            
            //Step3. Evaluate model with test data
            foreach (var test in testData)
            {
                // Use the code below to add input data
                var input = new ModelInput();
                input.Comment = test;

                // Try model on sample data
                // True is toxic, false is non-toxic
                ModelOutput result = predEngine.Predict(input);

                Console.WriteLine($"Text: {input.Comment} | Prediction: {(result.Score < 0 ? "Toxic" : "Non Toxic")} sentiment ({result.Score})");
            }
          
        }

        static void TSVMerge(string basepath)
        {
            Dictionary<string, annotation> aggression = new Dictionary<string, annotation>();
            string[] aggression_annotations_lines = System.IO.File.ReadAllLines($@"{basepath}\aggression_annotations.tsv");
            string[] aggression_annotated_comments_lines = System.IO.File.ReadAllLines($@"{basepath}\aggression_annotated_comments.tsv");
            for (int k=1;k<aggression_annotations_lines.Length;k++)
            {
                if(k%200 == 0)
                {
                    Console.Clear();
                    Console.WriteLine($"aggression_annotations_lines Line {k} of {aggression_annotations_lines.Length} - {((decimal)k*100/aggression_annotations_lines.Length).ToString("0.0")}% complete..\n");
                }

                string[] split = aggression_annotations_lines[k].Split("\t");
                if(split.Length == 4)
                {
                    if (!aggression.ContainsKey(split[0]))
                    {
                        aggression.Add(split[0], new annotation() { aggressionScore = decimal.Parse(split[3]), numOfVotes = 1, comment = aggression_annotated_comments_lines.FirstOrDefault(cmt => cmt.StartsWith(split[0])).Split("\t")[1] });
                    }
                    else
                    {
                        annotation ant = aggression[split[0]];
                        ant.aggressionScore += decimal.Parse(split[3]);
                        ant.numOfVotes += 1;
                        aggression[split[0]] = ant;
                    }
                }
            }

            System.IO.File.WriteAllText($@"{basepath}\aggression_merged.tsv", "aggression_score\tcomment\n");
            for (int k=0;k<aggression.Count;k++)
            {
                System.IO.File.AppendAllText($@"{basepath}\aggression_merged.tsv", $"{(aggression.ElementAt(k).Value.aggressionScore / aggression.ElementAt(k).Value.numOfVotes).ToString("0.0")}\t{aggression.ElementAt(k).Value.comment}\n");
            }
        }
    }
}
