﻿using MarvelHerois.helpers;
using MarvelHerois.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MarvelHerois.Services
{
    public class MarvelApiService
    {
        private static string[] HEROIS = new string[]
        {
            "Captain America", "Iron Man", "Thor", "Hulk",
            "Wolverine", "Spider-Man", "Black Panther",
            "Doctor Strange", "Daredevil"
        };

        public async Task<List<Personagem>> GetPersonagensAsync()
        {
            List<Personagem> personagens = new List<Personagem>();
            try
            {
                var httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string ts = DateTime.Now.Ticks.ToString();
                string publicKey = Constantes.PublicKey;
                string hash = SecurityHelper.GerarHash(ts, publicKey,
                                                        Constantes.PrivateKey);


                foreach (var heroi in HEROIS)
                {


                    var response = await httpClient.GetAsync(
                        Constantes.ApiBaseUrl +
                            $"characters?ts={ ts}&apikey={ publicKey}&hash={ hash}&" +
                        $"name={Uri.EscapeUriString(heroi)}").ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {

                        string conteudo =
                            response.Content.ReadAsStringAsync().Result;

                        dynamic resultado = JsonConvert.DeserializeObject(conteudo);

                        var personagem = new Personagem();

                        personagem.Nome = resultado.data.results[0].name;
                        personagem.Descricao = resultado.data.results[0].description;
                        personagem.UrlImagem = resultado.data.results[0].thumbnail.path + "." +
                            resultado.data.results[0].thumbnail.extension;
                        personagem.UrlWiki = resultado.data.results[0].urls[1].url;

                        personagens.Add(personagem);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return personagens;
        }
    }
}
