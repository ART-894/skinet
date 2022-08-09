using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ProductsController : ControllerBase
  {
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IGenericRepository<ProductBrand> _productBrandRepo;
    private readonly IGenericRepository<ProductType> _productTypeRepo;
    private readonly IMapper _mapper;

    public ProductsController(IGenericRepository<Product> productRepo, IGenericRepository<ProductType> productTypeRepo,
    IGenericRepository<ProductBrand> productBrandRepo, IMapper mapper)
    {
      _mapper = mapper;
      _productTypeRepo = productTypeRepo;
      _productBrandRepo = productBrandRepo;
      _productRepo = productRepo;

    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts()
    {
      var spec = new ProductWithTypesAndBrandsSpecification();

      var products = await _productRepo.ListAsync(spec);

      return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));
    }

    [HttpGet("{id}")]
    /*
    * On passing this id we create new instance of ProductWithTypesAndBrandsSpecification
    * We hit the parameterized constructor of the above class
    * We also create instance of BaseSpecification because ProductWithTypesAndBrandsSpecification 
      is inherited from BaseSpecification.
    * Next it hits the GetEntityWithSpec with spec in constructor in GenericRepository
    * public async Task<T> GetEntityWithSpec(ISpecification<T> spec) which in turn will hit the
    * private IQueryable<T> ApplySpecification(ISpecification<T> spec) 
    * where it hits the SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
    * _context.Set is the product db set 
    * and also the spec which is specification from ProductWithTypesAndBrandsSpecification
    * here we put the criteria in where clause like productID
    * then it hits aggregate of include methods like productBrand and Type
    * this returns a query, this IQueryable is then passed to the dbContext and in Db
    * this is finally returned in the method below.
    */
    public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
    {
      var spec = new ProductWithTypesAndBrandsSpecification(id);
      var product = await _productRepo.GetEntityWithSpec(spec);

      return _mapper.Map<Product, ProductToReturnDto>(product);
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
    {
      return Ok(await _productBrandRepo.ListAllAsync());
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
    {
      return Ok(await _productTypeRepo.ListAllAsync());
    }

  }
}