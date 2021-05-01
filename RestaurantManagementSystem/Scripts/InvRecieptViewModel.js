var ObjectState = {
    Unchanged: 0,
    Added: 1,
    Modified: 2,
    Deleted: 3
};
var InvReceiptDetailMaping = {
    'listDetail': {
        key: function (item) {
            return ko.utils.unwrapObservable(item.Id);
        },
        create: function (options) {
            return new InvReceiptDetailDto(options.data);
        }
    }
};
InvReceiptDetailDto = function (data) {
    var self = this;
    ko.mapping.fromJS(data, InvReceiptDetailMaping, self);

    self.flagInvReceiptDetailAsEdited = function () {
        console.log(self.ObjectState);
        if (self.ObjectState !== parseInt(ObjectState.Added)) {
            self.objectState = parseInt(ObjectState.Modified);
        }
        return true;
    };
};
InvReceiptDto = function (data) {
    var self = this;
    ko.mapping.fromJS(data, InvReceiptDetailMaping, self);

    if (self.objectState === parseInt(ObjectState.Added)) {
        self.ReceiptDate = moment(new Date()).utc().format('MM-DD-YYYY');
        self.ReceiptDate = moment(self.ReceiptDate());
    }
    else {
        self.ReceiptDate = moment(self.ReceiptDate());
    }

    self.save = function () {
        $.ajax({
            url: "/InvReceipts/SaveAsync",
            data: ko.toJSON(self),
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                console.log("checking edit id");
                console.log("/InvReceipts/Edit/" + data.result.Id);
                if (data.result.ObjectState === parseInt(ObjectState.Deleted)) {
                    toastr.success('Record Deleted!',
                        {
                            timeOut: 1000,
                            fadeOut: 1000
                        });
                    window.location.href = "/InvReceipts/Index";
                }
                if (data.result.ObjectState === parseInt(ObjectState.Added)) {
                    toastr.success('Record Added!');
                    window.location.href = "/InvReceipts/Edit/" + data.result.Id;
                }
                else if (data.result.ObjectState === parseInt(ObjectState.Modified)) {
                    toastr.success("Record Updated");

                }
                ko.mapping.fromJS(data.result, self);
            },
            error: function (err) {
                toastr.warning("Error updating database");
            }
        });
    };

    self.flagInvReceiptsAsEdited = function () {
        console.log(self);
        if (self.ObjectState !== parseInt(ObjectState.Added)) {
            self.ObjectState = parseInt(ObjectState.Modified);
        }
    };
    self.addChildItem = function () {
        var listitems = null;
        var issueNo = self.ReceiptNo;
        var issueId = self.Id;
        $.ajax({
            url: "/InvReceipts/ListItems",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                listitems = data.result;
            },
            error: function (err) {
                toastr.warning("Error updating database");
            },
            complete: function (data) {

                var item = new InvReceiptDetailDto({
                    DetailId: 0,
                    InvReceiptId: issueId,
                    InventoryItemId: 0,
                    Quantity: 0,
                    Price: 0,
                    ItemName: "",
                    ReceiptNo: issueNo,
                    ObjectState: parseInt(ObjectState.Added),
                    listItems: listitems
                });
                self.listDetail.push(item);
                console.log(self.listDetail());
            }
        });

    };
    self.deleteChildItem = function (invReceiptitem) {
        self.listDetail.remove(this);
        var id = invReceiptitem.DetailId();
        if (id > 0) {
            self.listDetailToDelete().push(id);
        }
        console.log(self.listDetailToDelete());
    };
};