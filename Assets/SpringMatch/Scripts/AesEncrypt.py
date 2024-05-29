# python -m pip install pycryptodome

from Crypto.Cipher import AES

with open("aes_key.bytes", "rb") as f:
    key = f.read()

with open("aes_iv.bytes", "rb") as f:
    iv = f.read()

aes = AES.new(key, AES.MODE_CBC, iv)

data = b"create a byte of give integer size"
size = (len(data) + 15)//16 * 16
data = data.ljust(size, b'\x00')

data = aes.encrypt(data)

with open("data.bytes", "wb") as f:
    f.write(data)

