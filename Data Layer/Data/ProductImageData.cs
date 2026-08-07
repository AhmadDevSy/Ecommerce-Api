using Models;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.Data
{
    public class ProductImageData
    {
        public static async Task<ProductImageDTO> GetById(int imageId)
        {
            return new ProductImageDTO();
        }

        public static async Task<AddEntityResult> Add(ProductImageDTO dto)
        {
            AddEntityResult result = new AddEntityResult();


            return result;
        }

        public static async Task<bool> Update(ProductImageDTO dto)
        {
            return true;
        }
    }
}
