using SignalRRestPOC.Web.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SignalRRestPOC.Web.Controllers {

    public class ToDo {

        public string ThingToDo { get; set; }
        public bool IsCompleted { get; set; }
        public string CompletedBy { get; set; }
        public DateTime CompletedOn { get; set; }
    }

    public class ToDosController : HubApiController<ToDoHub> {

        private static readonly ConcurrentDictionary<Guid, ToDo> _context = new ConcurrentDictionary<Guid, ToDo>(
            new List<KeyValuePair<Guid, ToDo>> { 
                new KeyValuePair<Guid, ToDo>(Guid.NewGuid(), new ToDo {  
                    ThingToDo = "Wash your clothes", IsCompleted = false
                })
            }
        );

        // GET api/todos
        public IEnumerable<ToDo> GetToDos() {

            return _context.Values;
        }

        public ToDo Get(Guid id) {

            ToDo toDo;
            _context.TryGetValue(id, out toDo);
            if (toDo == null) {

                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return toDo;
        }

        public HttpResponseMessage PostToDo(ToDo toDo) {

            Guid id = Guid.NewGuid();
            if (!_context.TryAdd(id, toDo)) {

                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            HubContext.Clients.All.toDoAdded(id, toDo);

            return Request.CreateResponse(HttpStatusCode.OK, toDo);
        }

        // PUT api/todos/1
        public ToDo PutToDo(Guid id, ToDo updatedToDo) {

            ToDo toDo;
            _context.TryGetValue(id, out toDo);
            if (toDo == null) {

                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _context.AddOrUpdate(id, updatedToDo, (_, __) => updatedToDo);
            HubContext.Clients.All.toDoUpdated(id, updatedToDo);
            return updatedToDo;
        }
    }
}