using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Domain.Enums
{
    public enum OrderStatus
    {
        // Estados posibles de una orden en el sistema de inventario.
        // 0 - Pendiente, 1 - Confirmada, 2 - Enviada, 3 - Entregada, 4 - Cancelada
        // Estos valores pueden ser utilizados para rastrear el estado de una orden a lo largo de su ciclo de vida.
        // Cada estado tiene un significado específico y puede influir en las acciones disponibles para la orden.

        /// <summary>
        /// Orden creada pero no confirmada.
        /// Puede ser modificada o cancelada.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Orden confirmada y lista para ser procesada.
        /// Stock reservado, no puede ser modificada.
        /// </summary>
        Confirmed = 1,

        /// <summary>
        /// Orden enviada al cliente.
        /// No puede ser cancelada.
        /// </summary>
        Shipped = 2,

        /// <summary>
        /// Orden entregada al cliente.
        /// Estado final. Exitoso.
        /// </summary>
        Delivered = 3,

        /// <summary>
        /// Orden cancelada por el cliente o el sistema.
        /// Estado final. No exitoso.
        /// </summary>
        Cancelled = 4
    }
}
