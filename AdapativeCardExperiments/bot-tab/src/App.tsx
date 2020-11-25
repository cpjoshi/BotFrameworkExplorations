import React from 'react';
import { Router, Route, Switch, Link, NavLink } from "react-router-dom";
import * as createHistory from "history";
import './App.css';
import configuration from './components/configuration';
import Introduction from './components/introduction';
import SSOPage from './components/ssopage';
import { MessageForm } from './components/MessageForm';
import { IFrameForm } from './components/IframeOnlyPage';

export const history = createHistory.createBrowserHistory();

function App() {
  return (
      <Router history={history}>
          <div>
              <Switch>
                  <Route path="/config" component={configuration} />
                  <Route path="/intro" component={Introduction} />
                  <Route path="/sso" component={SSOPage} />
                  <Route path="/message" component={MessageForm} />
                  <Route path="/iframepage" component={IFrameForm} />
              </Switch>
          </div>
      </Router>  );
}

export default App;
