%Matched Filter
function[h] = ch2(x,y)

Ny=length(y);
Nx=length(x);
L = Ny - Nx + 1;

while length(y) > L+length(x)-1
    % truncate
    y = y(1:length(y)-1);
end

while length(y) < L+length(x)-1
    % zero padding
    y = [y;0];
end

xr = flipud(x);
h = filter(xr,1,y);
h = h(Nx:end);
alpha = x'*x;
h = h/alpha;

end