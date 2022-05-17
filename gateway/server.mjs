import {ApolloGateway, RemoteGraphQLDataSource} from "@apollo/gateway";
import {ApolloServer} from "apollo-server";
import {HandleAuth} from "./handleAuth.mjs";
import fetch from 'node-fetch';
import { setTimeout } from 'timers/promises';

const services = [
    {name: 'persons', url: 'http://person:4001', status: false},
    {name: 'relationships', url: 'http://relationship:4002', status: false},
    {name: 'accounts', url: 'http://account:4003', status: false},
    {name: 'deposits', url: 'http://deposit:4004/graphql', status: false}
];

// Wait for services
while (services.filter(srv => !srv.status).length > 0) {
    services.forEach((srv, index) => {
        fetch(`${srv.url.replace("/graphql","")}/graphql?query=%7B__typename%7D`).then(res => {
            services[index].status = res.ok;
        }, err => console.error(err.message))
        .catch(err => console.error(err.message));
    });
    await setTimeout(3000);
}

console.log(`All services are OK:\n\r ${JSON.stringify(services, null, '\t')}`);

const gateway = new ApolloGateway({
    serviceList: services,
    buildService({name, url}) {
        return new RemoteGraphQLDataSource({
            url,
            willSendRequest({request, context}) {
                request.http.headers.set('authorization', context.payload);
            }
        });
    }
});

const auth = new HandleAuth();
const apollo = new ApolloServer({
    gateway: gateway,
    context: auth.handle,
    subscriptions: false,
    playground: true,
    introspection: true
});

apollo.listen({port: 4000, cors: { origin: '*' }}).then(({url}) => {
    console.log(`Server ready at ${url}`);
});