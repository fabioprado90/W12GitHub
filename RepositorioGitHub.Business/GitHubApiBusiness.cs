using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RepositorioGitHub.Business.Contract;
using RepositorioGitHub.Dominio;
using RepositorioGitHub.Dominio.Interfaces;
using RepositorioGitHub.Infra.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace RepositorioGitHub.Business
{
    public class GitHubApiBusiness : IGitHubApiBusiness
    {
        private readonly IContextRepository _context;
        private readonly IGitHubApi _gitHubApi;

        public string Url = "https://api.github.com/users/fabioprado90/repos";
        public int i = 0;
        public int count = 0;

        public GitHubApiBusiness(IContextRepository context, IGitHubApi gitHubApi)
        {
            _context = context;
            _gitHubApi = gitHubApi;
        }
        [HttpGet]
        public RepositoryViewModel Get()
        {
            using (var client = new HttpClient())
            {
                //prepara o HttpClient, incluindo o user-agent, sem isso a requisição não é autorizada
                client.BaseAddress = new Uri(Url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.ParseAdd("fabioprado90");

                HttpResponseMessage response = new HttpResponseMessage();

                try
                {
                    //faz a requisição para consumir a API do GitHub
                    response = client.GetAsync(Url).GetAwaiter().GetResult();
                }
                catch (HttpRequestException)
                {
                    response = null;
                }

                //retorna os dados em um texto Json
                var stream = response.Content.ReadAsStreamAsync().Result;
                var reader = new StreamReader(stream);
                string json = reader.ReadToEnd();

                //converte o json em um objeto GitHubRepository
                List<GitHubRepository> listRepositories = new
                    JavaScriptSerializer().Deserialize<List<GitHubRepository>>(json);

                RepositoryViewModel viewModel = new RepositoryViewModel();

                count = listRepositories.Count;
                viewModel.TotalCount = count;
                viewModel.Repositories = new GitHubRepository[listRepositories.Count];

                //preenche a viewModel com a lista de repositórios
                foreach (var item in listRepositories)
                {
                    viewModel.Repositories[i] = item;
                    i++;
                }

                return viewModel;
            }
        }
        [HttpGet]
        public GitHubRepository GetById(long id)
        {
            //chama o método Get e retorna a viewModel
            GitHubRepository repository = new GitHubRepository();
            var viewModel = this.Get();

            //seleciona o repositório na array de repositórios da viewModel de acordo com o id
            foreach (var item in viewModel.Repositories)
            {
                if (item.Id == id)
                {
                    repository = item;
                    break;
                }
            }

            return repository;
        }
        [HttpGet]
        public RepositoryViewModel GetByName(string name)
        {
            //chama o método Get e retorna a viewModel
            RepositoryViewModel repositoryViewModel = new RepositoryViewModel();
            var viewModel = this.Get();
            //instancia de um array de repositorios
            GitHubRepository[] repositories = new GitHubRepository[count];
            i = 0;

            //preenche o array de repositórios pegando os repositórios do viewModel
            foreach (var item in viewModel.Repositories)
            {
                if (item.Name.Contains(name))
                {
                    repositories[i] = item;
                    i++;
                }
            }

            //transforma o array em lista, para remover os itens nulos, depois transforma em array novamente
            List<GitHubRepository> list = new List<GitHubRepository>(repositories);
            list.RemoveAll(g => g == null);
            repositories = list.ToArray();
            
            repositoryViewModel.Repositories = repositories;

            return repositoryViewModel;
        }

        public ActionResult<FavoriteViewModel> GetFavoriteRepository()
        {
            return new ActionResult<FavoriteViewModel>();
        }

        public ActionResult<GitHubRepositoryViewModel> GetRepository(string owner, long id)
        {
            return new ActionResult<GitHubRepositoryViewModel>();
        }

        public ActionResult<FavoriteViewModel> SaveFavoriteRepository(FavoriteViewModel view)
        {
            return new ActionResult<FavoriteViewModel>();
        }
    }
}
