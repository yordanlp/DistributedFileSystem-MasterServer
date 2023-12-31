﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MasterServer;
using MasterServer.Models;

namespace MasterServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FilesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<File>>> GetFiles()
        {
            return await _context.Files.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<File>> GetFile(int id)
        {
            var file = await _context.Files.FindAsync(id);

            if (file == null)
            {
                return NotFound();
            }

            return file;
        }

        [HttpGet("GetByName/{fileName}")]
        public async Task<ActionResult<File>> GetByName(string fileName)
        {
            var file = await _context.Files.FirstOrDefaultAsync(f => f.Name == fileName);

            if (file == null)
            {
                return NotFound();
            }

            return file;
        }

        [HttpPost]
        public async Task<ActionResult<File>> PostFile(File file)
        {
            _context.Files.Add(file);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFile", new { id = file.Id }, file);
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            var file = await _context.Files.FirstOrDefaultAsync(file => file.Name == fileName);
            if (file == null)
            {
                return NotFound();
            }

            var chunks = await _context.Chunks.Where(chunk => chunk.File.Name == fileName).ToListAsync();

            _context.Chunks.RemoveRange(chunks);
            _context.Files.Remove(file);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
