﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using PI.CursoAngular.API.Models;
using AutoMapper;
using PI.CursoAngular.Repo.MariaDB.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using PI.CursoAngular.Repo.MariaDB.DBModelClientes;

namespace PI.CursoAngular.API.Controllers
{
    [Route("api/clientes")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IClientesRepository _cliRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IMapper mapper, 
                                  IClientesRepository cliRepository,
                                  IUnitOfWork unitOfWork,
                                  ILogger<ClientesController> logger) 
        {
            this._mapper = mapper;
            this._cliRepository = cliRepository;
            this._unitOfWork = unitOfWork;
            this._logger = logger;
        }

        [Authorize]
        [HttpPost]
        [Route("agregar")]
        public async Task<IActionResult> AgregarCliente(MCliente cli)
        {
            try
            {
                var cliAgregar = _mapper.Map<MCliente, Clientes>(cli);

                await _cliRepository.AgregarClienteAsync(cliAgregar);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500, e);
            }
        }


        [Authorize]
        [HttpGet]
        [Route("lista")]
        public async Task<IActionResult> Lista()
        {
            try
            {
                var clientes = await _cliRepository.ObtenerClientes();

                var retClientes = _mapper.Map<IEnumerable<Clientes>, IEnumerable<MCliente>>(clientes);

                return Ok(retClientes);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500, e);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("actualizar")]
        public async Task<IActionResult> ActualizarCliente(MCliente clienteCRUD)
        {
            try
            {
                // Recuperamos el cliente de la base
                Clientes clienteDB = await _cliRepository.ObtenerClientePorIdAsync(clienteCRUD.Id);

                if (clienteDB == null)
                {
                    return NotFound("El cliente no existe");
                }

                _mapper.Map(clienteCRUD, clienteDB);

                await _unitOfWork.CompleteAsync();

                var result = _mapper.Map<Clientes, MCliente>(clienteDB);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500, e);
            }
        }
    }
}