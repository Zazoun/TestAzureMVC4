using System.Collections.Generic;
using System.Web.Http;
using TestAnObject;

namespace TestMvc4.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly AnObjectRepository _list = new AnObjectRepository();

        private readonly AnObjectService _service = new AnObjectService();

        // GET api/values
        public IEnumerable<AnObject> Get()
        {
            return _list.GetAll();
        }

        // GET api/values/5
        public AnObject Get(int id)
        {
            return _list.Find(id);
        }

        // POST api/values
        public void Post(AnObject value)
        {
            _service.Add(value);
        }

        // PUT api/values/5
        public AnObject Put(int id, AnObject value)
        {
            return _service.Change(id, value);
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            _service.Delete(id);
        }

    }
}