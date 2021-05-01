var ObjectState = {
    Unchanged: 0,
    Added: 1,
    Modified: 2,
    Deleted: 3
};

var InvReturnsDetailMapping = {
    'listReturnDetails': {
        key: function (item) {
            return ko.utils.unwrapObservable(item.Id);
        }, 
        create: function (options) {
            return new InvReturnsDetailDto(options.data);
        }
    }
};
InvReturnsDetailDto = function (data) {
    var self = this;
    ko.mapping.fromJS(data, InvReturnsDetailMapping, self);

    self.flagInvReturnsDetailAsEdited = function () {
        console.log(self.ObjectState);
        if (self.ObjectState !== parseInt(ObjectState.Added)) {
            self.objectState = parseInt(ObjectState.Modified);
        }
        return true;
    };
};
InvReturnsDto = function (data) {
    var self = this;
    ko.mapping.fromJS(data, InvReturnsDetailMapping, self);

    if (self.objectState === parseInt(ObjectState.Added)) {
        self.ReturnDateTime = moment(new Date()).utc().format('MM-DD-YYYY');
        self.ReturnDateTime = moment(self.ReturnDateTime());
    }
    else {
        self.ReturnDateTime = moment(new Date(moment(self.ReturnDateTime()))).format('MM-DD-YYYY');// moment();
        //self.ReturnDateTime = moment(self.ReturnDateTime());
    }

    self.save = function () {
        self.ReturnDateTime = moment(new Date(moment(self.ReturnDateTime()))).format('DD-MM-YYYY');// moment();
        $.ajax({
            url: "/InvReturns/SaveAsync",
            data: ko.toJSON(self),
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data.result.objectState=== parseInt(ObjectState.Deleted) ) {
                    toastr.success('Record Deleted!',
                        {
                            timeOut: 1000,
                            fadeOut: 1000
                        });
                    window.location.href = "/InvReturns/Index";
                }
                if (data.result.objectState === parseInt(ObjectState.Added)) {
                    toastr.success('Record Added!');
                    window.location.href = "/InvReturns/Edit/" + data.result.Id;
                }
                else if (data.result.objectState === parseInt(ObjectState.Modified)) {
                    toastr.success("Record Updated");
                    
                }
                ko.mapping.fromJS(data.result, self);
            },
            error: function (err) {
                toastr.warning("Error updating database");
            }
        });
    };
    
    self.flagInvReturnsAsEdited = function () {
        console.log(self); 
        if (self.ObjectState !== parseInt(ObjectState.Added)) {
            self.objectState = parseInt(ObjectState.Modified);
        }
    };
    self.addChildItem =  function () {
        var listitems = null;
        var description = self.Description;
        var returnNo = self.ReturnNo;
        var returnId = self.Id;
        $.ajax({
            url: "/InvReturns/ListItems",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                listitems = data.result;
               
            },
            error: function (err) {
                toastr.warning("Error updating database");
            },
            complete: function (data){
              
                var item = new InvReturnsDetailDto({ Id: 0, InvReturnId: returnId, InventoryItemId: 0, Quantity: 0, ItemName: "", ReturnNo: returnNo, Description: description, objectState: parseInt(ObjectState.Added), listItems: listitems });
                self.listReturnDetails.push(item);
                console.log(self.listReturnDetails());
            }
        });
      
    };
    self.deleteChildItem = function (invReturnitem ) {
        self.listReturnDetails.remove(this);
        var id = invReturnitem.Id();
        if (id > 0 )
        {
            self.listReturnDetailsToDelete().push(id);
        }
        console.log(self.listReturnDetailsToDelete());
    };
};