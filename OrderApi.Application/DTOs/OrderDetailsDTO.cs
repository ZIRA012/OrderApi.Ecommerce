using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.DTOs
{
    public record OrderDetailsDTO(

        [Required] int OrdeID,//1
        [Required] int ProductID,//2
        [Required] int Client,//3
        [Required] string Name,//4
        [Required, EmailAddress] string Email,//5
        [Required] string Address,
        [Required] string TelephoneNumber,
        [Required] string ProductName,
        [Required] int PurchasedQuantity,
        [Required, DataType(DataType.Currency)] decimal Price,
        [Required, DataType(DataType.Currency)] decimal TotalPrice,
        [Required] DateTime OrderedDate
        );
   
}
