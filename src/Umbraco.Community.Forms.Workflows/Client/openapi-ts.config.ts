import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
  input: 'https://localhost:44368/umbraco/openapi/forms-community-management-workflows.json',
  output: {
    path: 'src/generated',
    // openapi-ts >=0.95 spawns post-processors as external binaries.
    // Prettier is a devDependency; eslint is intentionally not run on generated code.
    postProcess: ['prettier'],
  },
  plugins: [
    {
      name: '@hey-api/client-fetch',
      exportFromIndex: true,
      baseUrl: 'import.meta.env.VITE_APP_API_URL',
      throwOnError: true,
    },
    {
      name: '@hey-api/typescript',
      enums: 'typescript',
      readOnlyWriteOnlyBehavior: 'off',
    },
    {
      name: '@hey-api/sdk',
      asClass: true,
    },
  ],
});
