// await Bun.build({
//     entrypoints: ['./tiny.ts'],
//     target: 'bun',
//     minify: true,
//     plugins: [
//         {
//             name: 'remove-console',
//             setup(build) {
//                 build.onResolve({ filter: /.*/ }, () => ({
//                     // path: path.resolve(process.cwd(), 'dist/tiny.exe')
//                 }));
//             }
//         }
//     ]
// });