

namespace UP_Markov_Matveev
{
    using System;
    using System.Collections.Generic;
    
    public partial class Warehouses
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Warehouses()
        {
            this.Orders = new HashSet<Orders>();
        }
    
        public int WarehouseId { get; set; }
        public int CityId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Capacity { get; set; }
    
        public virtual Cities City { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
