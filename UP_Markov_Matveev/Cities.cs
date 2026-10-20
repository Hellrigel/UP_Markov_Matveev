

namespace UP_Markov_Matveev
{
    using System;
    using System.Collections.Generic;
    
    public partial class Cities
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cities()
        {
            this.Clients = new HashSet<Clients>();
            this.Warehouses = new HashSet<Warehouses>();
        }
    
        public int CityId { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Clients> Clients { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Warehouses> Warehouses { get; set; }
    }
}
