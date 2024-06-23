from Crypto.Cipher import AES

def read_file_bytes(fn):
    with open(fn, "rb") as f:
        return f.read()

def write_file_bytes(fn, data):
    with open(fn, "wb") as f:
        return f.write(data)

def pad(s):
    BS = AES.block_size
    return s + (BS - len(s) % BS) * bytes([BS - len(s) % BS])

key = read_file_bytes("aes_key.bytes")
iv = read_file_bytes("aes_iv.bytes")

def EncryptFile(srcFn, destFn):
    data = read_file_bytes(srcFn)
    data = pad(data)
    aes = AES.new(key, AES.MODE_CBC, iv)
    data = aes.encrypt(data)
    write_file_bytes(destFn, data)

def DecryptFile(srcFn, destFn):
    data = read_file_bytes(srcFn)
    aes = AES.new(key, AES.MODE_CBC, iv)
    data = aes.decrypt(data)
    write_file_bytes(destFn, data)
