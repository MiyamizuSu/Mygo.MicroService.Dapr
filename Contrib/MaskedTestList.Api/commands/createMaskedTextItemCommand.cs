using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RecAll.Contrib.MaskedTestList.Api.commands;


public class createMaskedTextItemCommand
{
    [Required] public string content { get; set; }
}