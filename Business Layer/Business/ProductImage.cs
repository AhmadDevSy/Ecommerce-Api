using Enums;
using Models.DTO;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Layer.Data;

namespace Business_Layer.Business
{
    public class ProductImage
    {
        private EnRecordMode Mode;


        public int Id { get; private set; }
        public string Url { get; set; }
        public int ProductId { get; set; }

        public ProductImageDTO DTO => new ProductImageDTO()
        {
            Id = this.Id,
            Url = this.Url,
            ProductId = this.ProductId,
        };


        public ProductImage()
        {
            Id = -1;
            Url = null!;
            ProductId = -1;

            Mode = EnRecordMode.Add;
        }

        private ProductImage(ProductImageDTO dto)
        {
            Id = dto.Id;
            Url = dto.Url;
            ProductId = dto.ProductId;

            Mode = EnRecordMode.Update;
        }

        public static async Task<ProductImage> GetById(int imageId)
        {
            ProductImageDTO dto = await ProductImageData.GetById(imageId);

            if (dto == null)
            {
                return null;
            }
            else
            {
                return new ProductImage(dto);
            }
        }


        public async Task<bool> Save()
        {
            switch (Mode)
            {
                case EnRecordMode.Add:
                    {
                        AddEntityResult addResult = await ProductImageData.Add(this.DTO);
                        if (addResult.Success)
                        {
                            this.Id = addResult.EntityId;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case EnRecordMode.Update:
                    {
                        return await ProductImageData.Update(this.DTO);
                    }
            }

            return false;
        }

        public static async Task<IEnumerable<ProductImageDTO>> GetAllByProductId(int productId)
        {
            return await ProductData.GetAllByProductId(productId);
        }
    }
}
