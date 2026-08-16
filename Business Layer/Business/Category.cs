
using Microsoft.Extensions.Caching.Memory;
using Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Data_Layer.Data;
using Enums;
using Models.DTO;


namespace Business_Layer.Business;

public class Category
{
    private EnRecordMode Mode;

    public int Id { get; protected set; }
    public string Name { get; set; }

    public CategoryDTO DTO => new CategoryDTO
    {
        Id = this.Id,
        Name = this.Name
    };

    public Category()
    {
        Id = 0;
        Name = null!;
    }

    private Category(CategoryDTO dto)
    {
        Id = dto.Id;
        Name = dto.Name;
        Mode = EnRecordMode.Update;
    }

    public static async Task<List<CategoryDTO>> GetAll()
    {
        return await CategoryData.GetAll();
    }

    public static async Task<Category> GetById(int id)
    {
        CategoryDTO dto = await CategoryData.GetById(id);

        if (dto == null)
        {
            return null;
        }
        else
        {
            return new Category(dto);
        }
    }

    public async Task<bool> Save()
    {
        switch (Mode)
        {
            case EnRecordMode.Add:
                {
                    int? id = await CategoryData.Add(this.DTO);

                    if (id != null)
                    {
                        this.Id = id.Value;
                        Mode = EnRecordMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            case EnRecordMode.Update:
                {
                    return await CategoryData.Update(this.DTO);
                }
        }

        return false;
    }

    public static async Task<bool> Exists(int categoryId)
    {
        return await CategoryData.Exists(categoryId);
    }

}
