clear

%Parameters
L = 0.5e-6;
C = 2e-12;
Z_l = 100;
Vmax = 1;
tw = 0.5e-9;
l = 1.2;
Z_0 = sqrt(L/C);
Gamma = (Z_l - Z_0)/(Z_l + Z_0);
gamma = sqrt(C*L);

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
                 - Gamma.^2 * U(t(j) + gamma * (x(i) - 4*l)) ...
                 + Gamma.^2 * U(t(j) - gamma * (x(i) + 4*l));
        I(i,j) = V(i,j)/Z_l;
    end
end

Bscan_plot(t, x, V, 't (s)', 'x (m)', 'u (v)', [20 45])
%Bscan_plot(t, x, I, 't (s)', 'x (m)', 'u (v)', [20 45])