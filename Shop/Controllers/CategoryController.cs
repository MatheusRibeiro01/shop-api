using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;


[Route("categories")]
public class CategoryController : ControllerBase
{
    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Category>>> Get(
        [FromServices] DataContext context
    )
    {
        var categories = await context.Categories.AsNoTracking().ToArrayAsync();
        return Ok(categories);

        //o AsNotTrackig limpa as informações adicionais que o entity trafega para que eu traga somente o que interessa
    }

    [HttpGet]
    [Route("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Category>> GetById(
        int id,
        [FromServices] DataContext context
        )
    {
         var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id== id);
         return Ok(category);
    }
    
    [HttpPost]
    [Route("")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> Post(
        [FromBody]Category model,
        [FromServices] DataContext context
    ) 
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Categories.Add(model);
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch (System.Exception)
        {
            return BadRequest(new {message = "Não foi possível criar a categoria"});
        }    
    }
    
    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> Put(
        int id,
        [FromBody] Category model,
        [FromServices] DataContext context)
    {
        //Verifica se o Id é o mesmo da model
        if (id != model.Id) 
            return NotFound(new {message = "Categoria não encontrada"});

        //Verifica se os dados são válidos
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Entry<Category>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new {message = "Esse registro já foi atualizado"});
        }
        catch (System.Exception)
        {
            return BadRequest(new {message = "Não foi possível atualizar esse registro"});
        }
    }
     
    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> Delete(
        int id,
        [FromServices] DataContext context)
    {
        var category = await context.Categories.FirstOrDefaultAsync(x => x.Id== id);
            if (category == null)
                return NotFound(new {message = "Categoria não encontrada"});

        try
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return Ok(new {message = "Categoria removida com sucesso"});
        }
        catch (System.Exception)
        {
            return BadRequest(new {message = "Não foi possível remover a categoria"});
        }        
    }
}