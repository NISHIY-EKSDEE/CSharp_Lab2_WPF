using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Lab2_WPF.model
{
    public class Threat
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string Object { get; set; }
        public string PrivacyViolation { get; set; }
        public string IntegrityViolation { get; set; }
        public string AvailabilityViolation { get; set; }

        public Threat() { }

        public Threat(long id, string name, string description, string source, string @object, string privacyViolation, 
            string integrityViolation, string availabilityViolation)
        {
            Id = id;
            Name = name;
            Description = description;
            Source = source;
            Object = @object;
            PrivacyViolation = privacyViolation;
            IntegrityViolation = integrityViolation;
            AvailabilityViolation = availabilityViolation;
        }

        public override bool Equals(object obj)
        {
            return obj is Threat threat &&
                   Id == threat.Id &&
                   Name == threat.Name &&
                   Description == threat.Description &&
                   Source == threat.Source &&
                   Object == threat.Object &&
                   PrivacyViolation == threat.PrivacyViolation &&
                   IntegrityViolation == threat.IntegrityViolation &&
                   AvailabilityViolation == threat.AvailabilityViolation;
        }

        public override int GetHashCode()
        {
            int hashCode = 83566971;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Source);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Object);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PrivacyViolation);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(IntegrityViolation);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AvailabilityViolation);
            return hashCode;
        }
    }


}
