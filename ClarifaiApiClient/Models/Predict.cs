using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClarifaiApiClient.Models
{
    public class Predict
    {
        public PredictStatus Status { get; set; }
        public List<PredictOutput> Outputs { get; set; }
    }

    public class PredictError
    {
        public PredictStatus Status { get; set; }
    }

    public class PredictStatus
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
    }

    public class PredictOutput
    {
        public string Id { get; set; }
        public PredictStatus Status { get; set; }
        public PredictData Data { get; set; }
        public string Created_At { get; set; }
        public PredictInput Input { get; set; }
    }

    public class PredictData
    {
        public List<PredictConcept> Concepts { get; set; }
    }
    public class PredictConcept
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public string App_Id { get; set; }
        public string Value { get; set; }

        public PredictClasses Prediction
        {
            get
            {
                if (String.IsNullOrWhiteSpace(Value))
                    return PredictClasses.Error;
                else
                {
                    try
                    {
                        Double v = Convert.ToDouble(Value);
                        if (v >= .45)
                            return PredictClasses.WithLogo;
                        else if (v < .45 && v >= .35)
                            return PredictClasses.Uncertain;
                        else
                            return PredictClasses.NoLogo;
                    }
                    catch
                    {
                        return PredictClasses.Error;
                    }
                }
            }
        }
    }
    public class PredictInput
    {
        public string Id { get; set; }
        public PredictImage Data { get; set; }
    }
    public class PredictImage
    {
        public PredictImageData Image { get; set; }
    }
    public class PredictImageData
    {
        public string Url { get; set; }
        public string Base64 { get; set; }
    }

    public enum PredictClasses
    {
        WithLogo,
        NoLogo,
        Uncertain,
        Error
    }
}
