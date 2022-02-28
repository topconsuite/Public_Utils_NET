using AutoMapper;

namespace Telluria.Utils.Crud.AutoMapper
{
  public static class Mapper
  {
    public static TTo Map<TFrom, TTo>(TFrom value)
    {
      var config = new MapperConfiguration(cfg => cfg.CreateMap<TFrom, TTo>());
      var mapper = new global::AutoMapper.Mapper(config);
      return mapper.Map<TTo>(value);
    }
  }
}
