
namespace UP_Markov_Matveev
{
    using System;
    using System.Collections.Generic;
    
    public partial class CargoTypes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CargoTypes()
        {
            this.Cargo = new HashSet<Cargo>();
        }
    
        public int CargoTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cargo> Cargo { get; set; }
    }
}
