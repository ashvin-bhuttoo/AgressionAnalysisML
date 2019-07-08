//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using Microsoft.ML.Data;

namespace AgressionAnalysisMLML.Model.DataModels
{
    public class ModelInput
    {
        [ColumnName("aggression_score"), LoadColumn(0)]
        public float Aggression_score { get; set; }


        [ColumnName("comment"), LoadColumn(1)]
        public string Comment { get; set; }


    }
}
