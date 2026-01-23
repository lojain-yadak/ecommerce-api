using KAShop.Dal.Data;
using KAShop.Dal.DTOs.Request;
using KAShop.Dal.DTOs.Response;
using KAShop.Dal.Models;
using KAShop.Dal.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAShop.Bll.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;

        public ProductService(IProductRepository productRepository,IFileService fileService)
        {
            _productRepository = productRepository;
            _fileService = fileService;
        }

        public async Task<ProductResponse> CreateProduct(ProductRequest request)
        {
            var product = request.Adapt<Product>();
            if (request.MainImage != null) { 
             var imagePath = await _fileService.UploadAsync(request.MainImage);
                product.MainImage = imagePath;
            }
            if(request.SubImages != null) {
                product.SubImages = new List<ProductImage>();
                foreach (var file in request.SubImages) { 
                  var imagePath = await _fileService.UploadAsync(file);
                    product.SubImages.Add(new ProductImage { 
                     ImageName=imagePath
                    });

                }
            }
            await _productRepository.AddAsync(product);
            var response =product.Adapt<ProductResponse>();
            response.SubImages=product.SubImages.Select(s=>s.ImageName).ToList();
            return response;

        }
        public async Task<List<ProductResponse>> GetAllProductsForAdmin()
        {
            var products = await _productRepository.GetAllAsync();

            var response = products.Adapt<List<ProductResponse>>();

            return response;
        }

    }
}
