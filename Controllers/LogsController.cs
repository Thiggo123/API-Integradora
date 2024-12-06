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
            if ((logEntradas == null || !logEntradas.Any()) && (logIds == null || !logIds.Any()))
            {
                return BadRequest("Nenhum log detectado. ");
            }

            var logsConvertidos = new List<string>();

            if (logEntradas != null && logEntradas.Any())
            {
                logsConvertidos.AddRange(ProcessarLogEntradas(logEntradas));

                if (download)
                {
                    Baixar(logsConvertidos);
                }

                return Ok(logsConvertidos);
            }
            if (logIds != null && logIds.Any())
            {
                return Ok(ProcessarLogIds(logIds));


            }
            return Ok(logsConvertidos);
        }

        private IEnumerable<string> ProcessarLogIds(List<int> logIds, [FromQuery] bool download = false)
        {
            var logsConvertidos = new List<string>();

            var logsdoBanco = _contexto.Logs.Where(x => logIds.Contains(x.Id)).ToList();

            if (logsdoBanco.Any())
            {

                foreach (var log in logsdoBanco)
                {
                    string resultado = $"\"MINHA CDN\" {log.Status}  {log.Tempo} {log.Codigo} {log.Acao} LogOriginal: {log.LogOriginal}";
                    logsConvertidos.Add(resultado);

                    log.LogConvertido = resultado;

                    _contexto.Logs.Update(log);
                }
            }
            _contexto.SaveChanges();

            if (!logsConvertidos.Any())
            {
                throw new Exception("nenhum LOG com este ID foi encontrado");
            }

            
            return logsConvertidos;
        }

        private IActionResult Baixar(List<string> logsConvertidos)
        {
            var arquivo = string.Join(Environment.NewLine, logsConvertidos);
            var arquivoNome = "logs_convertidos.txt";

            var arquivoBytes = System.Text.Encoding.UTF8.GetBytes(arquivo);

            return File(arquivoBytes, "text/plain", arquivoNome);
        }

        private IEnumerable<string> ProcessarLogEntradas(List<string> logEntradas)
        {
            var logsConvertidos = new List<string>();

            //Extrai as informações do JSON
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

                    //Salvo o novo log no banco, guardando também o JSON original e o Convertido
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
            return logsConvertidos;
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
