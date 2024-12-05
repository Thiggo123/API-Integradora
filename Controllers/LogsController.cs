using Microsoft.AspNetCore.Mvc;
using API_Integradora.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using static System.Net.Mime.MediaTypeNames;

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
        public IActionResult ConverterLogs([FromBody] List<string> logEntradas, [FromQuery] List<int> logIds = null, [FromQuery] bool download = false)
        {
            if ((logEntradas == null || !logEntradas.Any()) && logIds == null)
            {
                return BadRequest("Nenhum log detectado. ");
            }

            var logsConvertidos = new List<string>();

            if (logEntradas != null && logEntradas.Any())
            {
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
                        var req = detalhe.Substring(0, 4);
                        var tempo = Math.Round(double.Parse(partes[4].Trim()), 1);


                        string resultado = $"\"MINHA CDN\" {req} {status} {txt} {tempo} {codigo} {acao} ";

                        logsConvertidos.Add(resultado);

                        var novoLog = new Log
                        {
                            Codigo = codigo,
                            Status = status,
                            Acao = acao,
                            Tempo = tempo,
                            LogOriginal = log,
                            LogConvertido = resultado
                        };
                        _contexto.Logs.Add(novoLog);
                    }
                }


                _contexto.SaveChanges();

                if (download)
                {
                    var arquivo = string.Join(Environment.NewLine, logsConvertidos);
                    var arquivoNome = "logs_convertidos.txt";

                    var arquivoBytes = System.Text.Encoding.UTF8.GetBytes(arquivo);

                    return File(arquivoBytes, "text/plain", arquivoNome);
                }

                return Ok(logsConvertidos);
            }
            if (logIds != null && logIds.Any())
            {
                var logsdoBanco = _contexto.Logs.Where(x => logIds.Contains(x.Id)).ToList();

                if (!logsdoBanco.Any())
                {
                    return NotFound("Nenhum log com este ID foi encontrado");
                }
                foreach (var log in logsdoBanco)
                {
                    string resultado = $"\"MINHA CDN\" {log.Status}  {log.Tempo} {log.Codigo} {log.Acao} ";
                    logsConvertidos.Add(resultado);

                    log.LogConvertido = resultado;

                    _contexto.Logs.Update(log);
                }
                _contexto.SaveChanges();

                if (!logsConvertidos.Any())
                {
                    return BadRequest("nenhum log para converter");
                }

                if (download)
                {
                    var arquivo = string.Join(Environment.NewLine, logsConvertidos);
                    var arquivoNome = "logs_convertidos.txt";

                    var arquivoBytes = System.Text.Encoding.UTF8.GetBytes(arquivo);

                    return File(arquivoBytes, "text/plain", arquivoNome);
                }

                return Ok(logsConvertidos);


            }
            return Ok(logsConvertidos);
        }
        [HttpGet("recuperar")]
        public IActionResult RecuperarLog([FromQuery] List<int> logIds = null, [FromQuery] bool download = false)
        {
            if (logIds == null || !logIds.Any())
            {
                return NotFound("Nenhum log com este ID foi encontrado");
            }

            var logsRecuperados = _contexto.Logs.Where(x => logIds.Contains(x.Id)).ToList();

            if (download)
            {
                var arquivo = string.Join(Environment.NewLine, logsRecuperados);
                var arquivoNome = "logs_convertidos.txt";

                var arquivoBytes = System.Text.Encoding.UTF8.GetBytes(arquivo);

                return File(arquivoBytes, "text/plain", arquivoNome);
            }

            return Ok(logsRecuperados);

        }
    }
}
