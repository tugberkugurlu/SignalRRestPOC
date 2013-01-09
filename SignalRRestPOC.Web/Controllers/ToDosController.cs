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

        public int Id { get; set; }
        public string ThingToDo { get; set; }
        public bool IsCompleted { get; set; }
        public string CompletedBy { get; set; }
        public DateTime? CompletedOn { get; set; }
    }

    public class ToDosController : HubApiController<ToDoHub> {

        private static readonly ConcurrentDictionary<int, ToDo> _context = new ConcurrentDictionary<int, ToDo>(
            new List<KeyValuePair<int, ToDo>> { 
                new KeyValuePair<int, ToDo>(1, new ToDo { 
                    Id = 1, ThingToDo = "Wash your clothes", IsCompleted = false
                })
            }
        );

        // GET api/todos
        public IEnumerable<ToDo> GetToDos() {

            return _context.Values;
        }

        public ToDo Get(int id) {

            ToDo toDo;
            _context.TryGetValue(id, out toDo);
            if (toDo == null) {

                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return toDo;
        }

        public HttpResponseMessage PostToDo(ToDo toDo) {

            int id = _context.Max(x => x.Key) + 1;
            toDo.Id = id;
            if (!_context.TryAdd(id, toDo)) {

                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            HubContext.Clients.All.toDoAdded(toDo);

            return Request.CreateResponse(HttpStatusCode.OK, toDo);
        }

        // PUT api/todos/1
        public ToDo PutToDo(int id, ToDo updatedToDo) {

            ToDo toDo;
            _context.TryGetValue(id, out toDo);
            if (toDo == null) {

                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            if (!toDo.IsCompleted && updatedToDo.IsCompleted) {

                updatedToDo.CompletedBy = "User";
                updatedToDo.CompletedOn = DateTime.Now;
            }

            _context.AddOrUpdate(id, updatedToDo, (_, __) => updatedToDo);
            HubContext.Clients.All.toDoUpdated(id, updatedToDo);
            return updatedToDo;
        }
    }
}