MyStruct
    fld1: U16
U16: (self: MyStruct): U16
    return 42
    //return self.fld1
fn: (): U16
    s = MyStruct
        fld1 = 42
    return U16(s)
MyEnum: U8
    None = 0
