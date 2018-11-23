﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quien_es_Quien.Controllers
{
    public class JuegoController : Controller
    {
        public ActionResult Juego(string nombre = null)
        {
            BD.listaPersonajes = BD.ListarPersonajes(nombre);//Agregar para traer por categoria
            foreach (Personaje p in BD.listaPersonajes) //Cargar fotos en la carpetita
            {
                MemoryStream imgStream = new MemoryStream(p.Foto);
                Image img = Image.FromStream(imgStream);
                img.Save(Server.MapPath("~/Content/Fotos/" + p.IDPersonaje + ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);
            }

            Random rnd = new Random();
            int persElegido = BD.listaPersonajes[rnd.Next(BD.listaPersonajes.Count)].IDPersonaje;
            ViewBag.persElegido = persElegido;

            ViewBag.listaPreguntas = BD.ObtenerPreguntas(null);

            foreach (Personaje p in BD.listaPersonajes)
            {
                p.ListaPregs = BD.ObtenerPreguntasPersonaje(p.IDPersonaje);
            }

            return View();
        }

        public ActionResult ElegirModoJuego()
        {
            List<string> listaTipos = new List<string>();
            listaTipos = BD.ObtenerTiposPartida();
            ViewBag.ListaTipos = listaTipos;
            return View();
        }

        public ActionResult ElegirModo(string nombre)
        {
            string Action = "";
            BD.TipoPartida = nombre;
            switch (BD.TipoPartida)
            {
                case "SinglePlayer":
                    Action = "ElegirCategoria";
                    break;
                case "MultiPlayer":
                    Action = "Lobby";
                    break;
                case "Aprendiz":
                    break;
                case "Profesional":
                    break;
                case "Supremo":
                    break;
            }
            return RedirectToAction(Action, "Juego");
        }
        
        /*[HttpPost]

        public ActionResult ElegirModoJuego(string Tipo)
        {
            BD.TipoPartida = Tipo;
            return RedirectToAction("ElegirCategoria", "Juego");
        }*/

        public ActionResult ElegirCategoria()
        {
            BD.ObtenerCategoriasPersonajes();
            return View();
        }

        [HttpPost]
        public ActionResult ElegirCategoria(string Categoria)
        {
            BD.CategoriaJuego = Categoria;
            return RedirectToAction("Juego", "Juego");
        }

        [HttpPost]

        public ActionResult AgregarPartida(string Ganador, string Perdedor, int PuntosGanador, string TipoPartida)
        {
            BD.AgregarPartida(Ganador, Perdedor, PuntosGanador, TipoPartida);
            return RedirectToAction("holis"); //aca hay que cambiarlo porque no tengo idea a donde carajo va
        }

        public ActionResult Ganaste()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Ganaste(int score)
        {
            Session["puntaje"] = score;
            return View("Ganaste");
        }

        public ActionResult Perdiste()
        {
            return View();
        }

        public ActionResult Estadisticas()
        {
            Dictionary<string, int> dicTop10 = BD.RankingTop10();
            ViewBag.dicTop10 = dicTop10;
            int ID = BD.ObtenerIDUsuario(Session["nombre"].ToString());
            ViewBag.Jugadas = BD.CantidadJugadas(ID);
            ViewBag.Ganadas = BD.CantidadGanadas(ID);
            return View();
        }
    }
}