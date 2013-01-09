/// <reference path="jquery-1.8.3.intellisense.js" />
/// <reference path="knockout-2.2.0.debug.js" />
/// <reference path="jquery.signalR-1.0.0-rc1.js" />
/// <reference path="bootstrap.js" />

(function ($) {
    $(function () {

        function ToDo(id, thing, isComp, compBy, compOn) {

            var self = this;
            self.id = ko.observable(id);
            self.thingToDo = ko.observable(thing);
            self.isCompleted = ko.observable(isComp);
            self.completedBy = ko.observable(compBy);
            self.completedOn = ko.observable(compOn);

            self.completedText = ko.computed(function () {

                return "Completed @ " + self.completedOn() + " by " + self.completedBy();
            });

            self.markAsCompleted = function (todo) {

                todo.isCompleted(true);

                // put
                $.ajax({
                    type: "PUT",
                    contentType: "application/json",
                    url: "api/todos/" + todo.id(),
                    data: ko.toJSON(todo),
                    success: function (item) {
                        todo.completedBy(item.CompletedBy);
                        todo.completedOn(item.CompletedOn);
                    }
                });
            }
        }

        var toDoHub = $.connection.toDo;
        var viewModel = {
            toDos: ko.observableArray([]),
            addableToDo: ko.observable(new ToDo()),
            addToDo: function (todo) {

                $.ajax({
                    type: "POST",
                    contentType: "application/json",
                    url: "api/todos",
                    data: ko.toJSON(todo)
                });

                viewModel.addableToDo(new ToDo());
                $("#addNewDialog").modal("hide");
            },
            popUpAddNewDialog: function () {

                $("#addNewDialog").modal('show');
            }
        };

        ko.applyBindings(viewModel);

        $("#addNewDialog").on("hidden", function () {
            viewModel.addableToDo(new ToDo());
        });

        toDoHub.client.toDoAdded = function (todo) {

            console.log(todo);
            viewModel.toDos.push(new ToDo(todo.Id, todo.ThingToDo, todo.IsCompleted, todo.CompletedBy, todo.CompletedOn));
        }

        $.connection.logging = true;
        $.connection.hub.start().done(function () {

            $.getJSON("/api/todos", function (todos) {

                $.each(todos, function (i, todo) {
                    console.log(todo);
                    viewModel.toDos.push(new ToDo(todo.Id, todo.ThingToDo, todo.IsCompleted, todo.CompletedBy, todo.CompletedOn));
                });
            });
        });

    });
}(jQuery));