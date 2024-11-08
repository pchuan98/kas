import kaspa, { UtxoContext, UtxoProcessor } from "kaspa-chuan";
import { log } from "./log";

export class KaspaClient {
    readonly client: kaspa.RpcClient | null = null;

    readonly url: string;
    readonly network: string; // mainnet | testnet-10 | testnet-11 | devnet | simnet
    readonly encoding: string;

    private receivePrivateKey: string | null = null;
    private changePrivateKey: string | null = null;
    private receivePublicKey: string | null = null;
    private changePublicKey: string | null = null;

    private mnemonic: string | null = null;

    constructor(
        url: string = "vpn.pchuan.site",
        networkId: string = "testnet-10",
        encoding: string = "borsh"
    ) {
        this.url = url;
        this.network = networkId;
        this.encoding = encoding;

        this.client = new kaspa.RpcClient({
            url: this.url,
            networkId: this.network,
            encoding: this.encoding == "borsh"
                ? kaspa.Encoding.Borsh
                : kaspa.Encoding.SerdeJson,
        });

    }

    /**
     * connect and create client
     *
     * must call before use other methods
     */
    async connect(): Promise<void> {
        if (this.client == null) {
            log.error("client is not initialized");
            return;
        }

        await this.client.connect();
        log.info(this.client);
    }

    async getClient(): Promise<kaspa.RpcClient | null> {
        return this.client;
    }

    /**
     * disconnect and destroy client
     */
    async disconnect(): Promise<void> {
        if (this.client) {
            await this.client.disconnect();
        }

        log.info("kaspa client disconnected");
    }

    /**
     * get client status of getIsConnected
     * @returns is connected
     */
    async getIsConnected(): Promise<boolean> {
        return this.client?.isConnected ?? false;
    }

    /**
     * private key or mnemonic
     * @param str key
     */
    async setKey(str: string): Promise<void> {
        if (str.length == 64) // private key
            this.receivePrivateKey = str;
        else if (str.trim().split(/\s+/).length === 24) { // mnemonic
            this.mnemonic = str;

            const mnemonic = new kaspa.Mnemonic(str);
            const seed = mnemonic.toSeed();
            const xprv = new kaspa.XPrv(seed);
            this.receivePrivateKey = xprv.derivePath("m/44'/111111'/0'/0/0").toPrivateKey().toString();
            this.changePrivateKey = xprv.derivePath("m/44'/111111'/0'/1/0").toPrivateKey().toString();

            log.info("conveter mnemonic to private key");
        }
        else {
            log.error("invalid key");
        }

        if (this.network == null) {
            log.warn("network is not set");
            return;
        }

        // try to convert private key to public key
        try {
            this.receivePublicKey = new kaspa.PrivateKey(this.receivePrivateKey!)
                .toPublicKey()
                .toAddress(this.network)
                .toString();
            this.changePublicKey = new kaspa.PrivateKey(this.changePrivateKey!)
                .toPublicKey()
                .toAddress(this.network)
                .toString();
        }
        catch (e) {
            log.error(e);
        }
    }

    /**
     * create wallet
     * @param network network id
     */
    static async createWallet(network: string = "testnet-10"): Promise<{
        mnemonic: string;
        receivePrivateKey: string;
        changePrivateKey: string;
        receiveAddress: string;
        changeAddress: string;
    }> {
        const mnemonic = kaspa.Mnemonic.random();
        const seed = mnemonic.toSeed();
        const xprv = new kaspa.XPrv(seed);
        const receivePrivateKey = xprv.derivePath("m/44'/111111'/0'/0/0").toPrivateKey().toString();
        const changePrivateKey = xprv.derivePath("m/44'/111111'/0'/1/0").toPrivateKey().toString();

        const receivePublicKey = new kaspa.PrivateKey(receivePrivateKey)
            .toPublicKey()
            .toAddress(network)
            .toString();
        const changePublicKey = new kaspa.PrivateKey(changePrivateKey)
            .toPublicKey()
            .toAddress(network)
            .toString();

        log.info(`mnemonic: ${mnemonic.phrase}`);
        log.info(`receive private key: ${receivePrivateKey}`);
        log.info(`receive public key: ${receivePublicKey}`);
        log.info(`change private key: ${changePrivateKey}`);
        log.info(`change public key: ${changePublicKey}`);

        return {
            mnemonic: mnemonic.phrase,
            receivePrivateKey: receivePrivateKey,
            changePrivateKey: changePrivateKey,
            receiveAddress: receivePublicKey,
            changeAddress: changePublicKey,
        };
    }

    async getBalance(address: string | null = null): Promise<bigint> {
        address = address ?? this.receivePublicKey ?? "";
        log.info(`get balance of ${address}`);
        const balance = await this.client?.getBalanceByAddress({
            address: address,
        });

        return balance?.balance ?? 0n;
    }

    // async sendTransaction(address: string, amount: bigint): Promise<void> {
    //     const tx = await this.client?.submitTransaction({
    //         from: this.receivePublicKey!,
    //         to: address,
    //         amount: amount,
    //     });


    // }
}

const rpc = new kaspa.RpcClient({
    url: "vpn.pchuan.site",
    networkId: "testnet-10",
    encoding: kaspa.Encoding.Borsh,
});
await rpc.connect();

const info = await rpc.getServerInfo();
console.log(info);

// create sender
const processor = new UtxoProcessor({ rpc: rpc, networkId: "testnet-10" });
const context = new UtxoContext({ processor: processor });

const toAddr = "kaspatest:qp7uq35snucwdl8xyz8cj2r4s56sa0w3wpp0e4lg0ge5lcsqp4crj97gwl4s3";
const fromAddr = "kaspatest:qrcehetm28xr5w45jwmt35qqy36v7y6d4qw6zrpa9x80g6npdky8507dtnr3v";

processor.addEventListener("utxo-proc-start", async () => {
    log.info("utxo processor started");

    // clear
    log.info("clear");
    await context.clear();

    // tracking
    log.info("tracking");
    await context.trackAddresses([
        fromAddr
    ]);
});
processor.start();

// rpc.subscribeBlockAdded();

rpc.subscribeUtxosChanged([
    fromAddr,
    toAddr
]);

rpc.addEventListener("utxos-changed", async (event) => {
    // console.log("UTXO IN");
    // console.log(event.data.added);
    // console.log("UTXO OUT");
    // console.log(event.data.removed);
    // console.log("UTXO OFF");

    await new Promise(resolve => setTimeout(resolve, 10000));


    console.log((await rpc.getBalanceByAddress({ address: fromAddr })).balance);
    console.log((await rpc.getBalanceByAddress({ address: toAddr })).balance);
});


await new Promise(resolve => setTimeout(resolve, 1000));

const payload: kaspa.IPaymentOutput[] = [
    {
        address: toAddr,
        amount: kaspa.kaspaToSompi("1")!
    }
]

log.info((await rpc.getBalanceByAddress({ address: fromAddr })).balance);

// print blance
console.log((await rpc.getBalanceByAddress({ address: fromAddr })).balance);
console.log((await rpc.getBalanceByAddress({ address: toAddr })).balance);

const { transactions, summary } = await kaspa.createTransactions({
    outputs: payload,
    changeAddress: fromAddr,
    priorityFee: kaspa.kaspaToSompi("0.02"),
    entries: context,
});

console.log(kaspa.calculateTransactionFee("testnet-10", transactions[0].transaction));
// console.log(kaspa.calculateTransactionMass("testnet-10", transactions[0].transaction));

const trans = transactions[0];
trans.sign([new kaspa.PrivateKey("064e6af82ef976ff4bb6d5bbb50c6f7958bfb88cba952b0dd3bcd9ed9e80f348")]);
await trans.submit(rpc);

for (let i = 1; i < transactions.length; i++) {
    const transaction = transactions[i];
    console.log(`TrxManager: Payment with transaction ID: ${transaction.id} to be signed`);
    transaction.sign([new kaspa.PrivateKey("40c67afc725a0ef35e7f6808cf24f88b4b7287a3861682fec4180d1a2511d57c")]);
    await transaction.submit(rpc);
}

console.log(summary.finalTransactionId);


// print blance


console.log(summary.toJSON());

// await rpc.disconnect();


const wallet = new kaspa.Wallet({
    networkId: "testnet-10",
    encoding: kaspa.Encoding.Borsh,
});