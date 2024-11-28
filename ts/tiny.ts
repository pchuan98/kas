import kaspa from "kaspa-chuan";
import minimist from "minimist";
import { log } from "./utils/log"

// todo add args
const args = minimist(process.argv.slice(2));
const ticker = args.ticker || args.t;
const key = args.key;
const fee = args.fee || 0;
const gasFee = args.gas || 1;
const network = args.network || "mainnet";
const loops = args.loop || 1;
const url = args.url || "142.171.57.79";
const timeout = 60000;

// 添加帮助信息处理
if (args.help || args.h) {
    console.log(`
使用方法: bun run tiny.js [选项]

选项:
    --ticker, -t    代币标识符 (必需)
    --key          私钥 (必需)
    --fee          交易手续费 (默认: 0)
    --gas          Gas费用 (默认: 1)
    --network      网络类型 (默认: mainnet)
    --loop         循环次数 (默认: 1)
    --url          RPC节点URL (默认: 142.171.57.79)
    --help, -h     显示帮助信息

示例:
    bun run tiny.js --ticker KATS --key <your-private-key>
    bun run tiny.js -t KATS --key <your-private-key> --network testnet --loop 5
`);
    process.exit(0);
}

if (!ticker || !key) {
    log.error("缺少必需参数。必须设置 ticker 和 key. Use --help 或 -h 查看帮助信息");
    process.exit(1);
}

const client = new kaspa.RpcClient({
    url: url,
    encoding: kaspa.Encoding.Borsh,
    networkId: network,
});

await client.disconnect();
await client.connect();
log.info(`Connected to ${client}`);

// converter key
const privateKey = new kaspa.PrivateKey(key);
const publicKey = privateKey.toPublicKey();
const address = publicKey.toAddress(network);
log.info("converter key successfully created");

await client.subscribeUtxosChanged([address.toString()]);

let addedEventTrxId: any;
let SubmittedtrxId: any;
let eventReceived = false;

client.addEventListener('utxos-changed', async (event: any) => {
    log.info(`UTXO change event: ${address.toString()}`);

    const removedEntry = event.data.removed.find((entry: any) =>
        entry.address.payload === address.toString().split(':')[1]
    );
    const addedEntry = event.data.added.find((entry: any) =>
        entry.address.payload === address.toString().split(':')[1]
    );

    if (removedEntry) {
        log.info(`Removed UTXO found for address: ${address.toString()} with UTXO: ${JSON.stringify(removedEntry, (key, value) =>
            typeof value === 'bigint' ? value.toString() + 'n' : value)}`);
        log.info(`Removed UTXO found for address: ${address.toString()} with UTXO: ${JSON.stringify(removedEntry, (key, value) =>
            typeof value === 'bigint' ? value.toString() + 'n' : value)}`);

        addedEventTrxId = addedEntry.outpoint.transactionId;
        log.info(`Added UTXO TransactionId: ${addedEventTrxId}`);
        if (addedEventTrxId == SubmittedtrxId) {
            eventReceived = true;
        }
    } else {
        log.warn(`No removed UTXO found for address: ${address.toString()} in this UTXO change event`);
    }
});


const data = { "p": "krc-20", "op": "mint", "tick": ticker };
log.info(`Data: ${JSON.stringify(data)}`);

// <pubkey>
// OP_CHECKSIG
// OP_FALSE
// OP_IF
//  OP_PUSH "kasplex"
//  OP_PUSH 1
//  OP_PUSH "text/plain;charset=utf-8"
//  OP_PUSH 0
//  OP_PUSH "Hello, world!"
// OP_ENDIF

const script = new kaspa.ScriptBuilder()
    .addData(publicKey.toXOnlyPublicKey().toString())
    .addOp(kaspa.Opcodes.OpCheckSig)
    .addOp(kaspa.Opcodes.OpFalse)
    .addOp(kaspa.Opcodes.OpIf)
    .addData(new Uint8Array(Buffer.from("kasplex")))
    .addI64(1n)
    .addData(new Uint8Array(Buffer.from("text/plain;charset=utf-8")))
    .addI64(0n)
    .addData(new Uint8Array(Buffer.from(JSON.stringify(data, null, 0))))
    .addOp(kaspa.Opcodes.OpEndIf);
log.info(`Script: ${script.toString()}`);

const P2SHAddress = kaspa.addressFromScriptPublicKey(script.createPayToScriptHashScript(), network)!;
log.info(`P2SH Address: ${P2SHAddress.toString()}`);

for (let i = 0; i < loops; i++) {
    log.info(`Loop: ${i + 1}`);

    try {
        const { entries } = await client.getUtxosByAddresses({ addresses: [address.toString()] });
        const { transactions } = await kaspa.createTransactions({
            priorityEntries: [],
            entries,
            outputs: [{
                address: P2SHAddress.toString(),
                amount: kaspa.kaspaToSompi("0.1")!
            }],
            changeAddress: address.toString(),
            priorityFee: kaspa.kaspaToSompi(fee.toString())!,
            networkId: network
        });

        for (const transaction of transactions) {
            transaction.sign([privateKey]);
            log.info(`Main: Transaction signed with ID: ${transaction.id}`);
            const hash = await transaction.submit(client);
            log.info(`submitted P2SH commit sequence transaction on: ${hash}`);
            SubmittedtrxId = hash;
        }

        const commitTimeout = setTimeout(() => {
            if (!eventReceived) {
                log.error('Time out -> ReLoad');
            }
        }, timeout);

        while (!eventReceived) {
            await new Promise(resolve => setTimeout(resolve, 500)); // wait and check every 500ms
        }

        clearTimeout(commitTimeout);

        eventReceived = false;

        log.info(`creating UTXO entries from address ${address.toString()}`);
        const { entries: newEntries } = await client.getUtxosByAddresses({ addresses: [address.toString()] });
        log.info(`creating revealUTXOs from P2SHAddress ${P2SHAddress.toString()}`);
        const revealUTXOs = await client.getUtxosByAddresses({ addresses: [P2SHAddress.toString()] });
        log.info(`Creating Transaction with revealUT0s entries: ${revealUTXOs.entries[0]}`);

        const { transactions: revealTransactions } = await kaspa.createTransactions({
            priorityEntries: [revealUTXOs.entries[0]],
            entries: newEntries,
            outputs: [],
            changeAddress: address.toString(),
            priorityFee: kaspa.kaspaToSompi(gasFee.toString())!,
            networkId: network
        });

        let revealHash: any;
        for (const transaction of revealTransactions) {
            transaction.sign([privateKey], false);
            log.info(`Reveal: Transaction signed with ID: ${transaction.id}`);

            const ourOutput = transaction.transaction.inputs.findIndex((input) => input.signatureScript === '');
            if (ourOutput !== -1) {
                const signature = await transaction.createInputSignature(ourOutput, privateKey);
                transaction.fillInput(ourOutput, script.encodePayToScriptHashSignatureScript(signature));
            }
            revealHash = await transaction.submit(client);
            log.info(`submitted reveal tx sequence transaction: ${revealHash}`);
            SubmittedtrxId = revealHash;
        }

        const revealTimeout = setTimeout(() => {
            if (!eventReceived) {
                log.error("Timeout: Reveal transaction did not mature within 2 minutes");
                process.exit(1);
            }
        }, timeout);

        while (!eventReceived) {
            await new Promise(resolve => setTimeout(resolve, 500)); // wait and check every 500ms
        }

        clearTimeout(revealTimeout);
        eventReceived = false;

        const updatedUTXOs = await client.getUtxosByAddresses({ addresses: [address.toString()] });
        const revealAccepted = updatedUTXOs.entries.some(entry => {
            const transactionId = entry.entry.outpoint ? entry.entry.outpoint.transactionId : undefined;
            return transactionId === revealHash;
        });

        if (revealAccepted) {
            log.info(`Reveal transaction has been accepted: ${revealHash}`);
            if (i === loops - 1) {
                await client.disconnect();
                log.info("Client disconnected");
            }
        } else if (!eventReceived) {
            log.info("Reveal transaction has not been accepted yet.");
        }

    } catch (error) {
        log.error(`Error: ${error}`);
    }
}