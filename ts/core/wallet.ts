import kaspa from "kaspa-chuan";

/**
 * create mnemonic words
 * @returns mnemonic words string with 24 words
 */
export function createWallet(): string {
    const mnemonic = kaspa.Mnemonic.random();
    return mnemonic.phrase;
}

/**
 * get private key
 * @param mnemonic mnemonic words string with 24 words
 * @param account account number
 * @param change change number
 * @param index index number
 * @returns private key string
 */
function getPrivateKey(mnemonic: string, account = 0, change = 0, index = 0): string {
    if (!kaspa.Mnemonic.validate(mnemonic)) {
        throw new Error("invalid mnemonic");
    }

    const seed = new kaspa.Mnemonic(mnemonic).toSeed();
    const xprv = new kaspa.XPrv(seed);

    const privateKey = xprv.derivePath(`m/44'/111111'/${account}'/${change}/${index}`).toPrivateKey();

    return privateKey.toString();
}

/**
 * get receive private key
 * @param mnemonic mnemonic words string with 24 words
 * @param account account number
 * @param index index number
 * @returns receive private key string
 */
export function getReceivePrivateKey(mnemonic: string, account = 0, index = 0): string {
    return getPrivateKey(mnemonic, account, 0, index);
}

/**
 * get change private key
 * @param mnemonic mnemonic words string with 24 words
 * @param account account number
 * @param index index number
 * @returns change private key string
 */
export function getChangePrivateKey(mnemonic: string, account = 0, index = 0): string {
    return getPrivateKey(mnemonic, account, 1, index);
}

/**
 * get public key
 * @param privateKey private key string
 * @param network network name
 * @returns public key string
 */
export function getPublicKey(privateKey: string, network = "mainnet"): string {
    return new kaspa.PrivateKey(privateKey).toPublicKey().toAddress(network).toString();
}

const rpc = new kaspa.RpcClient({
    url: "127.0.0.1",
    networkId: "testnet-10",
    encoding: kaspa.Encoding.Borsh,
});
await rpc.connect();

const mnemonic = "cliff party catalog today outside catalog awake stem chair raw noise brother inspire utility fiber drastic tower stand carry enhance barrel strategy canoe ketchup";

// for (let i = 0; i < 10; i++) {
//     const key = getPublicKey(getReceivePrivateKey(mnemonic, 0, i), "testnet-10");
//     console.log(key);
//     console.log((await rpc.getBalanceByAddress({ address: key })).balance);
// }

console.log((await rpc.getUtxosByAddresses([getPublicKey(getReceivePrivateKey(mnemonic, 0, 1))])).entries.length);

rpc.disconnect();

