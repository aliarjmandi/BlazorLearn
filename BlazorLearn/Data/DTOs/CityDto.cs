﻿namespace BlazorLearn.Data.DTOs;

public class CityDto
{
    public int Id { get; set; }
    public int ProvinceId { get; set; }
    public string Name { get; set; } = string.Empty;
}
