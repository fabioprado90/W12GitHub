using Newtonsoft.Json;
using RepositorioGitHub.Dominio;
using RepositorioGitHub.Dominio.Interfaces;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositorioGitHub.Infra.ApiGitHub
{
    public class GitHubApi : IGitHubApi
    {
        public ActionResult<GitHubRepository> GetRepository(string owner)
        {
            throw new NotImplementedException();
        }

        public ActionResult<RepositoryModel> GetRepositoryByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
