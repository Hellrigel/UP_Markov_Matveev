
namespace UP_Markov_Matveev
{
    using System;
    using System.Collections.Generic;
    
    public partial class Orders
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Orders()
        {
            this.Cargo = new HashSet<Cargo>();
        }
    
        public int OrderId { get; set; }
        public int ClientId { get; set; }
        public Nullable<int> WarehouseId { get; set; }
        public int OrderStatusId { get; set; }
        public string PickupAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public System.DateTime OrderDate { get; set; }
        public Nullable<System.DateTime> PlannedDate { get; set; }
        public Nullable<System.DateTime> ActualDate { get; set; }
        public decimal Cost { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cargo> Cargo { get; set; }
        public virtual Clients Client { get; set; }
        public virtual OrderStatuses OrderStatus { get; set; }
        public virtual Warehouses Warehouse { get; set; }
    }
}
