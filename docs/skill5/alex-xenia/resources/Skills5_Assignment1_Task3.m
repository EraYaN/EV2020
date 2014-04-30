%Parameters
L_0 = 0.5e-6;
C_0 = 2e-12;
Z_l = 100;
Vmax = 1;
tw = 0.5e-9;
l = 1.2;
Z_0 = sqrt(L_0/C_0);
Gamma = (Z_l - Z_0)/(Z_l + Z_0);
gamma = sqrt(C_0*L_0);

%Vectors
t = 0:0.05e-9:5e-9;
x = 0:0.012:1.2;

t_length = length(t);
x_length = length(x);

V = zeros(x_length, t_length);
I = zeros(x_length, t_length);

U = @(t) Vmax * (heaviside(t) - heaviside(t - tw));

for i = 1:x_length
    for j = 1:t_length
        V(i,j) =              U(t(j) - gamma *  x(i)) ...
                 + Gamma    * U(t(j) + gamma * (x(i) - 2*l)) ...
                 - Gamma    * U(t(j) - gamma * (x(i) + 2*l)) ...
                 - Gamma^2 * U(t(j) + gamma * (x(i) - 4*l)) ...
                 + Gamma^2 * U(t(j) - gamma * (x(i) + 4*l));
        I(i,j) = (1/ Z_0)   * U(t(j) - gamma *  x(i)) ...
               -(Gamma/Z_0) * U(t(j) + gamma * (x(i) - 2*l)) ...
               -(Gamma/Z_0) * U(t(j) + gamma * (-x(i) -2*l)) ...
           + ((Gamma^2)/Z_0)* U(t(j) + gamma * (x(i) -4*l)) ;
    end
end

%Bscan_plot(t, x, V, 't (s)', 'x (m)', 'U (v)', [20 45])
Bscan_plot(t, x, I, 't (s)', 'x (m)', 'I (A)', [20 45])