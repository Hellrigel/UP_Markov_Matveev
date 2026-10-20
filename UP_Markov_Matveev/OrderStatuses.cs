

namespace UP_Markov_Matveev
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderStatuses
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderStatuses()
        {
            this.Orders = new HashSet<Orders>();
        }
    
        public int OrderStatusId { get; set; }
        public string Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
