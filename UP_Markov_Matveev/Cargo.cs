

namespace UP_Markov_Matveev
{
    using System;
    using System.Collections.Generic;
    
    public partial class Cargo
    {
        public int CargoId { get; set; }
        public int OrderId { get; set; }
        public int CargoTypeId { get; set; }
        public string Name { get; set; }
        public decimal WeightKg { get; set; }
        public decimal VolumeM3 { get; set; }
    
        public virtual CargoTypes CargoType { get; set; }
        public virtual Orders Orders { get; set; }
    }
}
