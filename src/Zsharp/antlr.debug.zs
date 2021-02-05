MyStruct
    fld1: U16
fn: (self: MyStruct): U16
    x.call()
    return self.fld1
s = MyStruct
    fld1 = 42
s.fn()