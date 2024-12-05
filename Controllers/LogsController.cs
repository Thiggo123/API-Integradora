using Microsoft.AspNetCore.Mvc;
using API_Integradora.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace API_Integradora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly Contexto _contexto;

        public LogsController(Contexto contexto)
        {
            _contexto = contexto;
        }
        [HttpPost("converter")]
        public IActionResult ConverterLogs([FromBody] List<string> logEntradas)
        {
            if (logEntradas == null || !logEntradas.Any()) {
                return BadRequest("Nenhum log detectado. ");
        }
            var logsConvertidos = new List<string>();

            foreach (var log in logEntradas)
            {
                var partes = log.Split('|');
                if (partes.Length == 5)
                {
                    var codigo = int.Parse(partes[0].Trim());
                    var status = int.Parse(partes[1].Trim());
                    var acao = partes[2].Trim('"');
                    var detalhe = partes[3].Trim('"');
                    var txt = detalhe.Substring(4, 11);
                    var req = detalhe.Substring(0,4);
                    var tempo = Math.Round(double.Parse(partes[4].Trim()), 1);


                    string resultado = $"\"MINHA CDN\" {req} {status} {txt} {tempo} {codigo} {acao} ";

                    logsConvertidos.Add(resultado);

                    var novoLog = new Log
                    {
                        Codigo = codigo,
                        Status = status,
                        Acao = acao,
                        Tempo = tempo
                    };
                    _contexto.Logs.Add(novoLog);
                }
            }
            _contexto.SaveChanges();
            return Ok(logsConvertidos);

        } } }
